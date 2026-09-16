import { ProblemDetails } from './problem-details';

// Mirrors GlobalExceptionHandler's ValidationProblemDetails response (FluentValidation failures via ValidationBehavior)
export interface ValidationProblemDetails extends ProblemDetails {
  errors: Record<string, string[]>;
}

export function isValidationProblemDetails(body: unknown): body is ValidationProblemDetails {
  return !!body && typeof body === 'object' && 'errors' in (body as Record<string, unknown>);
}