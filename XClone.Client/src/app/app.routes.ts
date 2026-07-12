import { Routes } from '@angular/router';
import {Register} from './features/auth/register/register';
import {Login} from './features/auth/login/login';
import {FeedComponent} from './features/feed-component/feed-component';
import {authGuardGuard} from './core/guards/auth-guard-guard';
import {guestGuardGuard} from './core/guards/guest-guard-guard';

export const routes: Routes = [
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'login', component: Login, canActivate: [guestGuardGuard] },
  { path: 'register', component: Register, canActivate: [guestGuardGuard] },
  { path: 'main', component: FeedComponent, canActivate: [authGuardGuard] }
];
