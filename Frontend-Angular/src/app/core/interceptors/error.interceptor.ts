import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpErrorResponse
} from '@angular/common/http';
import { catchError, throwError } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private snackBar: MatSnackBar) {}

  intercept(req: HttpRequest<any>, next: HttpHandler) {

    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {

        let message = 'Unexpected error';

        if (error.error?.detail) {
          message = error.error.detail;
        } else if (error.status === 404) {
          message = 'Not found';
        } else if (error.status === 400) {
          message = 'Bad request';
        } else if (error.status === 500) {
          message = 'Server error';
        }

        this.snackBar.open(message, 'Close', {
          duration: 4000,
          panelClass: ['error-snackbar']
        });

        return throwError(() => error);
      })
    );
  }
}