import { Venue } from "@/types/Venue";
import { ScheduledClass } from "@/types/ScheduledClass";

interface AddClassRequest {
  venueId: string;
  classId: string;
  emailAddress: string;
  startDateTime: string;
}

interface AddClassResponse {
  requiresVerification: boolean;
}

const baseURL = import.meta.env.VITE_API_BASEURL;

async function fetchVenues(): Promise<Venue[]> {
  const response = await fetch(`${baseURL}/get-venues`);
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }

  const data: Venue[] = await response.json();
  return data;
}

async function fetchScheduledClasses(venueId: string, startDate: string): Promise<ScheduledClass[]> {
  const response = await fetch(
    `${baseURL}/get-classes?venueId=${venueId}&startDate=${encodeURIComponent(startDate)}`
  );
  if (!response.ok) {
    throw new Error(`HTTP error! status: ${response.status}`);
  }
  return response.json();
}

async function addClass(requestData: AddClassRequest): Promise<AddClassResponse> {
  const response = await fetch(`${baseURL}/add-classes`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Accept: "*/*",
    },
    body: JSON.stringify(requestData),
  });

  if (!response.ok) {
    if (response.status === 400) {
      const message = await response.text();
      if (message && message.length < 200 && !message.trimStart().startsWith("{")) {
        throw new Error(message);
      }
    }

    throw new Error("Unable to add the class alert. Please try again later.");
  }

  return response.json();
}

async function verifyEmail(token: string): Promise<void> {
  const response = await fetch(`${baseURL}/verify-email`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ token }),
  });

  if (!response.ok) {
    throw new Error(
      response.status === 400
        ? "This verification link is invalid or has expired."
        : "We could not verify your email. Please try again later."
    );
  }
}

async function unsubscribe(token: string): Promise<void> {
  const response = await fetch(`${baseURL}/unsubscribe`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ token }),
  });

  if (!response.ok) {
    throw new Error(
      response.status === 400
        ? "This unsubscribe link is invalid."
        : "We could not unsubscribe you. Please try again later."
    );
  }
}

export { fetchScheduledClasses, addClass, fetchVenues, verifyEmail, unsubscribe };
