import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {apiUrl} from '../../../environments/environment';
import {Observable} from 'rxjs';

export interface ITweet{
  id: string;
  text: string;
  createdAt: Date;
  userId: string;
}

@Injectable({
  providedIn: 'root',
})
export class TweetService {
  private readonly http: HttpClient =  inject(HttpClient);

  getAllTweets(): Observable<ITweet[]> {
    return this.http.get<ITweet[]>(`${apiUrl}/api/Tweets`);
  }

  createTweet(Text: string): Observable<ITweet> {
    return this.http.post<ITweet>(`${apiUrl}/api/Tweets`, {text: Text})
  }

}
