import { HttpErrorResponse } from '@angular/common/http';
import { ApiResult } from './models';

const GENERIC_ERROR = 'Ocurrió un error inesperado. Intenta de nuevo.';

export function errorMessage(err: unknown): string {
  if (!(err instanceof HttpErrorResponse)) return GENERIC_ERROR;

  if (err.status === 0) {
    return 'No se pudo conectar con la API. Verifica que esté en ejecución.';
  }

  if (err.status >= 500) return GENERIC_ERROR;

  const body = err.error as ApiResult<unknown> | null;

  if (body?.error?.code === 'ERROR_DE_VALIDACION' && body.message) return body.message;

  return body?.error?.description ?? GENERIC_ERROR;
}
