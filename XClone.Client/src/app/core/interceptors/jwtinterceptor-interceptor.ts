import {HttpInterceptorFn, HttpRequest} from '@angular/common/http';

export const jWTInterceptorInterceptor: HttpInterceptorFn = (req, next) => {

  const jwt = localStorage.getItem('token');

  if (jwt) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${jwt}`
      }
    });
  }

  return next(req);
};
