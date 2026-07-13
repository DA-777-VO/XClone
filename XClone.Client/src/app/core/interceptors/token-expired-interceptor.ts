import {HttpErrorResponse, HttpInterceptorFn} from '@angular/common/http';
import {Router} from '@angular/router';
import {inject} from '@angular/core';
import {catchError} from 'rxjs';

export const tokenExpiredInterceptor: HttpInterceptorFn = (req, next) => {
  const router: Router = inject(Router)
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      localStorage.removeItem('token');
      console.log(error);
      if (error.status === 401) {
        router.navigate(['/login']);
      }
      throw error;
    })
  );
};
