import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class ApiErrorService {
  message(error: unknown, fallback = 'Something went wrong. Please try again.'): string {
    if (error instanceof HttpErrorResponse) {
      const body = error.error;
      if (typeof body === 'string' && body.trim()) return body;
      if (body?.message) return body.message;
      if (Array.isArray(body?.errors)) return body.errors.join(', ');
      if (body?.errors && typeof body.errors === 'object') {
        const values = Object.values(body.errors).flat().filter(Boolean);
        if (values.length) return values.join(', ');
      }
      if (error.status === 0) return 'Cannot reach the API. Check that the ASP.NET backend is running and the API URL is correct.';
      if (error.status === 401) return 'Your session has expired. Please sign in again.';
      if (error.status === 403) return 'You do not have permission to perform this action.';
      if (error.status === 409) return body?.message || 'This action conflicts with the latest data. The page has been refreshed.';
    }
    return fallback;
  }
}
