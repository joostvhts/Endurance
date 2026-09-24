import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { AlertCircle, CheckCircle2, LoaderCircle } from "lucide-react";
import { unsubscribe } from "@/services/apiService";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "./ui/dialog";
import { Button } from "./ui/button";

type UnsubscribeStatus = "confirm" | "loading" | "success" | "error";

interface UnsubscribeDialogProps {
  token: string | null;
  onClose: () => void;
}

export default function UnsubscribeDialog({ token, onClose }: UnsubscribeDialogProps) {
  const navigate = useNavigate();
  const [status, setStatus] = useState<UnsubscribeStatus>("confirm");
  const [message, setMessage] = useState("This will stop all current class alerts for your email address.");

  useEffect(() => {
    if (!token) {
      setStatus("error");
      setMessage("This unsubscribe link is invalid.");
      return;
    }

    window.history.replaceState(null, "", `${window.location.pathname}${window.location.search}`);
  }, [token]);

  const close = () => {
    onClose();
    navigate("/", { replace: true });
  };

  const handleUnsubscribe = async () => {
    if (!token) return;

    setStatus("loading");
    setMessage("Stopping your class alerts...");
    try {
      await unsubscribe(token);
      setStatus("success");
      setMessage("You have been unsubscribed and all your class alerts have been stopped.");
    } catch (error: unknown) {
      setStatus("error");
      setMessage(error instanceof Error ? error.message : "We could not unsubscribe you.");
    }
  };

  return (
    <Dialog open onOpenChange={(open) => !open && close()}>
      <DialogContent className="font-geist dark:border-zinc-800 dark:bg-zinc-900 dark:text-white">
        <DialogHeader className="items-center text-center">
          {status === "confirm" && <AlertCircle className="h-10 w-10 text-zinc-500" />}
          {status === "loading" && <LoaderCircle className="h-10 w-10 animate-spin text-zinc-500" />}
          {status === "success" && <CheckCircle2 className="h-10 w-10 text-green-600" />}
          {status === "error" && <AlertCircle className="h-10 w-10 text-red-500" />}
          <DialogTitle>
            {status === "confirm" && "Unsubscribe from all alerts?"}
            {status === "loading" && "Unsubscribing"}
            {status === "success" && "Unsubscribed"}
            {status === "error" && "Unsubscribe failed"}
          </DialogTitle>
          <DialogDescription aria-live="polite">{message}</DialogDescription>
        </DialogHeader>
        {status === "confirm" && (
          <DialogFooter className="gap-2 sm:space-x-0">
            <Button variant="ghost" onClick={close}>
              Keep my alerts
            </Button>
            <Button variant="destructive" onClick={handleUnsubscribe}>
              Unsubscribe
            </Button>
          </DialogFooter>
        )}
        {(status === "success" || status === "error") && (
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
