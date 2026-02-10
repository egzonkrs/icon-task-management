import axios from "axios";
import type { ApiResponse } from "@/types";

const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || "http://localhost:5000/api/v1",
  withCredentials: true,
  headers: {
    "Content-Type": "application/json",
  },
});

apiClient.interceptors.request.use(
  (config) => {
    return config;
  },
  (error) => Promise.reject(error)
);

apiClient.interceptors.response.use(
  (response) => {
    const apiResponse = response.data as ApiResponse<unknown>;
    if (apiResponse && apiResponse.isFailed) {
      const errorMessage = apiResponse.errors
        ? Object.values(apiResponse.errors).join(", ")
        : "An unexpected error occurred";
      return Promise.reject(new ApiClientError(errorMessage, apiResponse.errors));
    }
    return response;
  },
  (error) => {
    if (axios.isAxiosError(error) && error.response) {
      const apiResponse = error.response.data as ApiResponse<unknown> | undefined;
      const status = error.response.status;

      if (status === 401) {
        window.dispatchEvent(new CustomEvent("auth:unauthorized"));
      }

      const errorMessage = apiResponse?.errors
        ? Object.values(apiResponse.errors).join(", ")
        : error.message;

      return Promise.reject(new ApiClientError(errorMessage, apiResponse?.errors ?? null, status));
    }
    return Promise.reject(error);
  }
);

export class ApiClientError extends Error {
  public errors: Record<string, string> | null;
  public status: number | undefined;

  constructor(
    message: string,
    errors: Record<string, string> | null | undefined,
    status?: number
  ) {
    super(message);
    this.name = "ApiClientError";
    this.errors = errors ?? null;
    this.status = status;
  }
}

export default apiClient;
