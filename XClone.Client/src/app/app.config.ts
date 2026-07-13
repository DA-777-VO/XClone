import { ApplicationConfig, provideBrowserGlobalErrorListeners } from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import {provideHttpClient, withInterceptors} from '@angular/common/http';
import {jWTInterceptorInterceptor} from './core/interceptors/jwtinterceptor-interceptor';
import {tokenExpiredInterceptor} from './core/interceptors/token-expired-interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withInterceptors([jWTInterceptorInterceptor, tokenExpiredInterceptor]))
  ]
};
