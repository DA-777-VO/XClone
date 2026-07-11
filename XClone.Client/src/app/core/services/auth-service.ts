import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { apiUrl } from '../../../environments/environment';
import { map, Observable, tap } from 'rxjs';

export interface ILoginData {
  email: string;
  password: string;
}

export interface registerData {
  username: string;
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private http = inject(HttpClient);

  login(loginData: ILoginData): Observable<string> {
    return this.http
      .post<LoginResponse>(`${apiUrl}/api/Auth/login`, loginData)
      .pipe(
        tap((response) => this.saveToken(response.token)),
        map((response) => response.token),
      );
  }

  register(registerData: registerData): Observable<string> {
    return this.http.post<string>(`${apiUrl}/api/Auth/register`, registerData);
  }

  saveToken(token: string) {
    localStorage.setItem('token', token);
  }
}
