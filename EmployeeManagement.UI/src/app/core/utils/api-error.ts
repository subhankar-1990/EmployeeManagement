import { ApiException, ProblemDetails } from '../models/employee.model';

/** Field name -> messages, as produced by the API's FluentValidation pipeline. */
export type ApiFieldErrors = Readonly<Record<string, readonly string[]>>;

const UNEXPECTED_MESSAGE = 'Something went wrong while talking to the API. Please try again.';
const NETWORK_MESSAGE =
  'The API could not be reached. Check that the API is running and that the browser can reach it.';

/**
 * Normalised failure surfaced by `EmployeeService`.
 *
 * The generated client throws two different shapes: an `ApiException` when a response had no JSON
 * body, and the deserialised `ProblemDetails` (plus a `traceId` and an `errors` dictionary for
 * validation failures) when it had one. Both are folded into this single type.
 */
export class ApiError extends Error {
  /** HTTP status code, or `null` when the request never reached the API. */
  readonly status: number | null;

  /** Field level messages keyed by the server property name, e.g. `Name`. */
  readonly fieldErrors: ApiFieldErrors;

  /** Correlation id written by the API, when it supplied one. */
  readonly traceId: string | null;

  constructor(
    message: string,
    status: number | null = null,
    fieldErrors: ApiFieldErrors = {},
    traceId: string | null = null,
  ) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.fieldErrors = fieldErrors;
    this.traceId = traceId;
  }

  /** True when the API rejected the payload and reported which fields were at fault. */
  get hasFieldErrors(): boolean {
    return Object.keys(this.fieldErrors).length > 0;
  }
}

/** Maps any thrown value onto an `ApiError`. Already normalised errors are returned unchanged. */
export function toApiError(error: unknown): ApiError {
  if (error instanceof ApiError) {
    return error;
  }

  if (typeof error === 'object' && error !== null && ApiException.isApiException(error)) {
    const problem = parseProblemDetails(error.response);
    return new ApiError(
      messageForStatus(error.status, asString(problem?.['detail']), asString(problem?.['title']), asString(problem?.['traceId'])),
      error.status,
      readFieldErrors(problem),
      asString(problem?.['traceId']),
    );
  }

  if (error instanceof ProblemDetails) {
    const problem = error as unknown as Record<string, unknown>;
    const status = typeof problem['status'] === 'number' ? problem['status'] : null;
    return new ApiError(
      messageForStatus(status, asString(problem['detail']), asString(problem['title']), asString(problem['traceId'])),
      status,
      readFieldErrors(problem),
      asString(problem['traceId']),
    );
  }

  if (error instanceof Error) {
    // `fetch` rejects with a TypeError when the request never made it to the server.
    return new ApiError(error.name === 'TypeError' ? NETWORK_MESSAGE : error.message || UNEXPECTED_MESSAGE);
  }

  return new ApiError(UNEXPECTED_MESSAGE);
}

/** Human readable message for any thrown value. */
export function errorMessageOf(error: unknown): string {
  return toApiError(error).message;
}

/** Field level messages for any thrown value, keyed by the server property name. */
export function fieldErrorsOf(error: unknown): ApiFieldErrors {
  return toApiError(error).fieldErrors;
}

function parseProblemDetails(body: string): Record<string, unknown> | null {
  if (!body) {
    return null;
  }

  try {
    const parsed: unknown = JSON.parse(body);
    return typeof parsed === 'object' && parsed !== null ? (parsed as Record<string, unknown>) : null;
  } catch {
    return null;
  }
}

function readFieldErrors(problem: Record<string, unknown> | null): ApiFieldErrors {
  const errors: unknown = problem?.['errors'];
  if (typeof errors !== 'object' || errors === null) {
    return {};
  }

  const result: Record<string, readonly string[]> = {};
  for (const [field, messages] of Object.entries(errors as Record<string, unknown>)) {
    if (Array.isArray(messages)) {
      const values = messages.filter((message): message is string => typeof message === 'string');
      if (values.length > 0) {
        result[field] = values;
      }
    } else if (typeof messages === 'string' && messages.length > 0) {
      result[field] = [messages];
    }
  }

  return result;
}

function asString(value: unknown): string | null {
  return typeof value === 'string' && value.length > 0 ? value : null;
}

function messageForStatus(
  status: number | null,
  detail: string | null,
  title: string | null,
  traceId: string | null,
): string {
  switch (status) {
    case 400:
      return detail ?? title ?? 'The request was rejected. Review the values you entered and try again.';
    case 403:
      return detail ?? 'You are not permitted to perform this operation.';
    case 404:
      // `NotFound()` from the controller returns the generic "Not Found" title, so only a
      // `detail` (present in Development) is more informative than the message below.
      return detail ?? 'The employee could not be found. It may have been deleted by someone else.';
    case 409:
      return detail ?? title ?? 'The request conflicts with the current state of the data.';
    case 429:
      return 'Too many requests were sent to the API. Wait a moment and try again.';
    default:
      break;
  }

  if (status === null) {
    return title ?? UNEXPECTED_MESSAGE;
  }

  if (status >= 500) {
    const message = detail ?? 'The API reported an unexpected error. Please try again in a moment.';
    return traceId ? `${message} (trace id: ${traceId})` : message;
  }

  return detail ?? title ?? `The request failed with status ${status}.`;
}
