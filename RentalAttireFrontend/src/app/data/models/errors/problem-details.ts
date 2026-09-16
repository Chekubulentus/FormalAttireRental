// Mirrors GlobalExceptionHandler's plain ProblemDetails response (KeyNotFoundException, 500s)
export interface ProblemDetails {
  status: number;
  title: string;
  detail: string;
  instance: string;
}