import { ProblemDetails } from '../models/errors/problem-details';
import { ValidationProblemDetails } from '../models/errors/validation-problem-details';
import { isValidationProblemDetails } from '../models/errors/validation-problem-details';

/**
 * Extracts a human-readable message from an HttpErrorResponse's `error` body,
 * which can now be a ValidationProblemDetails, a plain ProblemDetails, or
 * (for any endpoint not yet migrated to ToActionResult) a raw string.
 */
export function extractErrorMessage(errorBody: unknown): string {
  if (typeof errorBody === 'string') {
    return errorBody;
  }

  if (isValidationProblemDetails(errorBody)) {
    // Flatten all field-level messages into one readable string.
    // e.g. "Status: Invalid transition. RentalId: Rental not found."
    const messages = Object.entries((errorBody as ValidationProblemDetails).errors)
      .map(([field, msgs]) => `${field}: ${msgs.join(' ')}`);
    return messages.join(' ');
  }

  if (errorBody && typeof errorBody === 'object' && 'detail' in errorBody) {
    return (errorBody as ProblemDetails).detail;
  }

  return 'An unexpected error occurred.';
}