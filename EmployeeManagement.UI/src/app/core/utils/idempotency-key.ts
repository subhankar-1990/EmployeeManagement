/**
 * Creates the value sent in the `Idempotency-Key` header of `POST /api/v1/employees`.
 *
 * The API stores the first response for a key and replays it for retries, so a resubmitted form
 * can never create a second employee. The key is generated once per form submission.
 */
export function createIdempotencyKey(): string {
  const cryptoApi: Crypto | undefined = globalThis.crypto;
  if (cryptoApi && typeof cryptoApi.randomUUID === 'function') {
    return cryptoApi.randomUUID();
  }

  const random = Math.random().toString(36).slice(2, 12);
  return `employee-${Date.now().toString(36)}-${random}`;
}
