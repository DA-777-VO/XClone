import {Component, inject} from '@angular/core';
import {AuthService, IRegisterData} from '../../../core/services/auth-service';
import {FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {Router} from '@angular/router';

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './register.html',
  styleUrl: './register.css',
})
export class Register {

  authService = inject(AuthService);
  router = inject(Router);

  registerForm: FormGroup = new FormGroup({
    username: new FormControl('', Validators.required) ,
    email: new FormControl('', Validators.required),
    password: new FormControl('', Validators.required)
  })

  onSubmit() {
    if (this.registerForm.invalid) return;

    const registerData: IRegisterData = this.registerForm.value;
    this.authService.register(registerData).subscribe({
      next: data => {
        this.router.navigate(['/login']);
      },
      error: err => {
        console.log(err);
      }
    })
  }

}
