import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { AlertCircle, CheckCircle2, LoaderCircle } from "lucide-react";
import { verifyEmail } from "@/services/apiService";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "./ui/dialog";
import { Button } from "./ui/button";

type VerificationStatus = "loading" | "success" | "error";

const verificationRequests = new Map<string, Promise<void>>();

interface VerifyEmailDialogProps {
  token: string | null;
  onClose: () => void;
}

export default function VerifyEmailDialog({ token, onClose }: VerifyEmailDialogProps) {
  const navigate = useNavigate();
  const [status, setStatus] = useState<VerificationStatus>("loading");
  const [message, setMessage] = useState("Verifying your email address...");

  useEffect(() => {
    if (!token) {
      setStatus("error");
      setMessage("This verification link is invalid or has expired.");
      return;
    }

    window.history.replaceState(null, "", `${window.location.pathname}${window.location.search}`);
    let active = true;

    let request = verificationRequests.get(token);
    if (!request) {
      request = verifyEmail(token);
      verificationRequests.set(token, request);
    }

    request
      .then(() => {
        if (!active) return;
        setStatus("success");
        setMessage("Your email is verified and your class alert is now active.");
      })
      .catch((error: unknown) => {
        if (!active) return;
        setStatus("error");
        setMessage(error instanceof Error ? error.message : "We could not verify your email.");
      })
      .finally(() => verificationRequests.delete(token));

    return () => {
      active = false;
    };
  }, [token]);

  const close = () => {
    onClose();
    navigate("/", { replace: true });
  };

  return (
    <Dialog open onOpenChange={(open) => !open && close()}>
      <DialogContent className="font-geist dark:border-zinc-800 dark:bg-zinc-900 dark:text-white">
        <DialogHeader className="items-center text-center">
          {status === "loading" && <LoaderCircle className="h-10 w-10 animate-spin text-zinc-500" />}
          {status === "success" && <CheckCircle2 className="h-10 w-10 text-green-600" />}
          {status === "error" && <AlertCircle className="h-10 w-10 text-red-500" />}
          <DialogTitle>
            {status === "loading" && "Verifying email"}
            {status === "success" && "Email verified"}
            {status === "error" && "Verification failed"}
          </DialogTitle>
          <DialogDescription aria-live="polite">{message}</DialogDescription>
        </DialogHeader>
        {status !== "loading" && (
          <DialogFooter>
            <Button className="w-full sm:w-auto" onClick={close}>
              Continue to Endurance
            </Button>
          </DialogFooter>
        )}
      </DialogContent>
    </Dialog>
  );
}
