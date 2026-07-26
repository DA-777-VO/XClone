import {Component, signal, inject, OnInit} from '@angular/core';
import {ITweet, TweetService} from '../../core/services/tweet-service';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {Router, RouterLink} from '@angular/router';
import {AuthService} from '../../core/services/auth-service';

@Component({
  selector: 'app-feed-component',
  imports: [
    ReactiveFormsModule, RouterLink
  ],
  templateUrl: './feed-component.html',
  styleUrl: './feed-component.css',
})
export class FeedComponent implements OnInit {
  private readonly tweetService: TweetService =  inject(TweetService)
  private readonly authService: AuthService =  inject(AuthService)
  private readonly router: Router =  inject(Router)

  tweets = signal<ITweet[]>([])
  activeTab = signal<'all' | 'feed'>('all')

  tweetForm: FormGroup = new FormGroup({
    tweet: new FormControl('', Validators.required)
  })

  ngOnInit() {
    this.loadTweets();
  }

  loadTweets() {
    const request$ = this.activeTab() === 'all'
      ? this.tweetService.getAllTweets()
      : this.tweetService.getHomeFeed();

    request$?.subscribe({
      next: (tweets) => {
        this.tweets.set(tweets),
        console.log(this.tweets());
      },
      error: err => console.log(err)
    })
  }

  switchTab(tab: 'all' | 'feed') {
    this.activeTab.set(tab);
    this.loadTweets();
  }


  onSubmitTweet(){
    if(this.tweetForm.invalid) return;

    const Text: string = this.tweetForm.value.tweet;
    console.log('Sending tweet:', { text: Text });

    this.tweetService.createTweet(Text).subscribe({
      next: tweet => {
        if (this.activeTab() === 'all')
        {
          this.tweets.update(tweets => [tweet, ...tweets]);
        }
        this.tweetForm.reset();
      },
      error: err => console.log(err)
    })
  }

  onClickLogout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
