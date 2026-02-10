export interface ApiResponse<TData> {
  data: TData | null;
  isFailed: boolean;
  isSuccess: boolean;
  reasons: Record<string, string> | null;
  errors: Record<string, string> | null;
}

export interface ApiError {
  code: string;
  message: string;
}
