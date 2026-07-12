import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { apiUrl } from '../../../environments/environment';
import { map, Observable, tap } from 'rxjs';

export interface ILoginData {
  email: string;
  password: string;
}

export interface IRegisterData {
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

  register(registerData: IRegisterData): Observable<string> {
    return this.http.post<string>(`${apiUrl}/api/Auth/register`, registerData);
  }

  saveToken(token: string) {
    localStorage.setItem('token', token);
  }

  logout() {
    localStorage.removeItem('token');
  }


  isAuthenticated(): boolean {
    const token = localStorage.getItem('token');
    if (!token) return false;

    try {
      const payloadBase64 = token.split('.')[1];
      const decodedPayload = JSON.parse(atob(payloadBase64));

      console.log('payload', decodedPayload)

      if (!decodedPayload.exp) return true;

      console.log('expiryTime', decodedPayload.exp)

      const expiryTime = decodedPayload.exp * 1000;

      console.log(Date.now())
      return Date.now() < expiryTime;

    } catch (e) {
      return false;
    }
  }

}
