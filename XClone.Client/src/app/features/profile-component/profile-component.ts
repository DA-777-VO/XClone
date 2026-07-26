import {Component, inject, OnInit, signal} from '@angular/core';
import {IUserProfile, UserService} from "../../core/services/user-service";
import { AuthService } from "../../core/services/auth-service";
import {ActivatedRoute, Router, RouterLink} from "@angular/router";

@Component({
  selector: 'app-profile-component',
  imports: [RouterLink],
  templateUrl: './profile-component.html',
  styleUrl: './profile-component.css',
})
export class ProfileComponent implements OnInit {
  private readonly userService: UserService = inject(UserService);
  private readonly authService: AuthService = inject(AuthService);
  private readonly route: ActivatedRoute = inject(ActivatedRoute);
  private readonly router: Router = inject(Router);

  profile = signal<IUserProfile | null>(null);
  isMyProfile = signal<boolean>(false);


  ngOnInit() {
    this.route.paramMap.subscribe(param => {
      const profileUsername = param.get('username');
      const myUsername = this.authService.getCurrentUsername();


      if (!profileUsername || profileUsername === myUsername) {
        this.isMyProfile.set(true)
        this.loadMyProfile()
      } else {
          this.isMyProfile.set(false)
          this.loadUserProfile(profileUsername)
      }
    })
  }

  loadMyProfile() {
    this.userService.getMyProfile().subscribe({
      next: (profile) => {
        console.log(profile);
        this.profile.set(profile);
      },
      error: (err) => console.log(err)
    })
  }

  loadUserProfile(username: string) {
    this.userService.getUserProfile(username).subscribe({
      next: (profile) => {
        console.log(profile)
        this.profile.set(profile);
      },
      error: (error) => console.log(error)
    })
  }


  onFollowToggle() {
    const currentProfile = this.profile();
    if (!currentProfile) return;

    this.userService.toggleFollow(currentProfile.id).subscribe({
      next: (response) => {
        console.log(response),
        this.loadUserProfile(currentProfile.username);
      },
      error: (err) => console.log(err)
    })

  }
  onLogout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
