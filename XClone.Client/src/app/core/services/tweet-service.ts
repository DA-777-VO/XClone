import {inject, Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {apiUrl} from '../../../environments/environment';
import {Observable} from 'rxjs';

export interface ITweet{
  id: string;
  text: string;
  createdAt: Date;
  authorId: string;
  authorName: string;

	// ToDO finish up \ figure out
  likesCount: number;
  isLiked: boolean;
}

export interface LikeResponse {
  message: string;
  likesCount: number;
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

  getHomeFeed(): Observable<ITweet[]> {
    return this.http.get<ITweet[]>(`${apiUrl}/api/Tweets/feed`);
  }

  toggleLike(tweetId: string): Observable<LikeResponse> {
    return this.http.post<LikeResponse>(`${apiUrl}/api/Tweets/${tweetId}/like`, {});
  }
}
