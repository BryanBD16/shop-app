export function localDateTimeToUtcIso(localDateTime: string): string {
  return new Date(localDateTime).toISOString();
}

export function utcIsoToLocalDateTimeInput(value?: string | null): string {
  if (!value) {
    return '';
  }

  const date = parseUtcDate(value);
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  const hours = String(date.getHours()).padStart(2, '0');
  const minutes = String(date.getMinutes()).padStart(2, '0');

  return `${year}-${month}-${day}T${hours}:${minutes}`;
}

function parseUtcDate(value: string): Date {
  const hasTimezone = /([zZ]|[+-]\d\d:\d\d)$/.test(value);

  return hasTimezone ? new Date(value) : new Date(`${value}Z`);
}