import {Component, signal, inject} from '@angular/core';
import {ITweet, TweetService} from '../../core/services/tweet-service';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {Router} from '@angular/router';
import {AuthService} from '../../core/services/auth-service';

@Component({
  selector: 'app-feed-component',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './feed-component.html',
  styleUrl: './feed-component.css',
})
export class FeedComponent {
  private readonly tweetService: TweetService =  inject(TweetService)
  private readonly authService: AuthService =  inject(AuthService)
  private readonly router: Router =  inject(Router)
  tweets = signal<ITweet[]>([])

  tweetForm: FormGroup = new FormGroup({
    tweet: new FormControl('', Validators.required)
  })

  ngOnInit() {
    this.tweetService.getAllTweets().subscribe({
      next: tweets => {
        this.tweets.set(tweets);
        console.log(this.tweets());
      },
      error: err =>
        console.log(err)
      }

    )
  }

  onSubmitTweet(){
    if(this.tweetForm.invalid) return;

    const Text: string = this.tweetForm.value.tweet;
    console.log('Sending tweet:', { text: Text });

    this.tweetService.createTweet(Text).subscribe({
      next: tweet => {
        this.tweets.update(tweets => [tweet, ...tweets]);
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
