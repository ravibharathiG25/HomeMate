import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const token = authService.token();

  let authReq = req;

  // Attach Bearer token if available and request is targeting the API
  if (token && !req.url.includes('/login') && !req.url.includes('/register') && !req.url.includes('/refresh-token')) {
    authReq = req.clone({
      setHeaders: {
        Authorization: `Bearer ${token}`
      }
    });
  }

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      // If 401 Unauthorized and not already on an auth endpoint
      if (error.status === 401 && !req.url.includes('/login') && !req.url.includes('/refresh-token')) {
        return authService.refreshToken().pipe(
          switchMap(newAuth => {
            const retryReq = req.clone({
              setHeaders: {
                Authorization: `Bearer ${newAuth.token}`
              }
            });
            return next(retryReq);
          }),
          catchError(refreshError => {
            authService.logout();
            return throwError(() => refreshError);
          })
        );
      }

      return throwError(() => error);
    })
  );
};
