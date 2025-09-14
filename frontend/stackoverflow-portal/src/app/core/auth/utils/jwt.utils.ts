export const TOKEN_KEY = 'token';

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY);
}

function base64UrlDecode(input: string): string {
  input = input.replace(/-/g, '+').replace(/_/g, '/');
  const pad = input.length % 4;
  if (pad) input += '='.repeat(4 - pad);
  return atob(input);
}

export function decodeJwt<T = any>(token: string): T | null {
  try {
    const payload = token.split('.')[1];
    if (!payload) return null;
    const json = base64UrlDecode(payload);
    return JSON.parse(json) as T;
  } catch {
    return null;
  }
}

export function isJwtExpired(token: string, skewSeconds = 30): boolean {
  const payload = decodeJwt<{ exp?: number; nbf?: number }>(token);
  if (!payload?.exp) return true;
  const now = Math.floor(Date.now() / 1000);
  if (payload.nbf && now + skewSeconds < payload.nbf) return true;
  return payload.exp <= now + skewSeconds;
}
