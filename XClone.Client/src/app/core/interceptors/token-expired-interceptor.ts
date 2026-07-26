import {HttpErrorResponse, HttpInterceptorFn} from '@angular/common/http';
import {Router} from '@angular/router';
import {inject} from '@angular/core';
import {catchError} from 'rxjs';

export const tokenExpiredInterceptor: HttpInterceptorFn = (req, next) => {
  const router: Router = inject(Router)
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      console.log('Произошла ошибка запроса:', error);
      if (error.status === 401) {
        console.warn('Сессия устарела. Разлогиниваем...');
        localStorage.removeItem('token');
        router.navigate(['/login']);
      }
      throw error;
    })
  );
};
