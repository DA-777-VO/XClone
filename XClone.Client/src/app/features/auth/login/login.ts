import { Component, inject } from '@angular/core';
import { AuthService, ILoginData } from '../../../core/services/auth-service';
import {Router, RouterModule} from '@angular/router';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private authService = inject(AuthService);
  private router = inject(Router);

  loginForm: FormGroup = new FormGroup({
    email: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required),
  });


  onSubmit() {
    if (this.loginForm.invalid) return;

    const loginData = this.loginForm.value as ILoginData;

    this.authService.login(loginData).subscribe({
      next: (token) => {
        console.log(token)
        this.router.navigate(['/main'])
      },

      error: (err) => {
        console.log(err);
        this.router.navigate(['/'])
      }
    })
  }

  // onSubmit(){
  //   this.authService.getAllTweets().subscribe({
  //     next: (tweets) => {
  //       console.log(tweets);
  //     },
  //     error: (err) => {
  //       console.log('Ошибка:', err);
  //     }
  //   });
  // }

}
