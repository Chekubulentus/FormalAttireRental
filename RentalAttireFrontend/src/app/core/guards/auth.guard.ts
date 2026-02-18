import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth-service/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  var accessToken = authService.getAccessToken();
  var user = authService.getCurrentUser();

  if(accessToken)
    return true;

  router.navigateByUrl('/log-in');
  return false;
};
