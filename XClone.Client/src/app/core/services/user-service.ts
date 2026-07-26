import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {apiUrl} from '../../../environments/environment';
import {Observable} from 'rxjs';

export interface IUserProfile {
  id: string;
  username: string;
  bio: string | null;
  tweetsCount: number;
  followersCount: number;
  followingCount: number;
}

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private readonly http: HttpClient =  inject(HttpClient);


  getMyProfile(): Observable<IUserProfile> {
    return this.http.get<IUserProfile>(`${apiUrl}/api/User/me`);
  }

  getUserProfile(username: string): Observable<IUserProfile> {
    return this.http.get<IUserProfile>(`${apiUrl}/api/User/${username}`);
  }

  toggleFollow(followeeId: string): Observable<string> {
    return this.http.post(`${apiUrl}/api/User/${followeeId}/follow`, {}, {responseType: 'text'});
  }
}
