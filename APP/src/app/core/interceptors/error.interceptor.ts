import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { StorageService } from '../services/storage.service';
import { NotificationService } from '../services/notification.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const storageService = inject(StorageService);
  const notificationService = inject(NotificationService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      let errorMessage = 'Ocorreu um erro inesperado';

      if (error.error instanceof ErrorEvent) {
        // Erro do lado do cliente
        errorMessage = `Erro: ${error.error.message}`;
      } else {
        // Erro do lado do servidor
        switch (error.status) {
          case 401:
            // Apenas desloga se:
            // 1. Há um token armazenado (usuário estava logado)
            // 2. E não é uma tentativa de login (que pode falhar com credenciais erradas)
            const hasToken = !!storageService.getToken();
            const isLoginRequest = req.url.includes('/auth/login');
            
            if (hasToken && !isLoginRequest) {
              // Token expirado/inválido - desloga
              storageService.clearAll();
              router.navigate(['/login']);
              errorMessage = 'Sessão expirada. Por favor, faça login novamente.';
              notificationService.error(errorMessage);
            } else if (isLoginRequest) {
              // Falha no login - não desloga, apenas retorna erro
              errorMessage = error.error?.message || 'Credenciais inválidas';
              notificationService.error(errorMessage);
            } else {
              // Sem token, redireciona para login
              router.navigate(['/login']);
              errorMessage = 'Você precisa fazer login para acessar este recurso.';
              notificationService.warning(errorMessage);
            }
            break;
          case 403:
            errorMessage = 'Você não tem permissão para acessar este recurso.';
            notificationService.error(errorMessage);
            break;
          case 404:
            errorMessage = 'Recurso não encontrado.';
            notificationService.error(errorMessage);
            break;
          case 500:
            errorMessage = 'Erro interno do servidor. Tente novamente mais tarde.';
            notificationService.error(errorMessage);
            break;
          default:
            errorMessage = error.error?.message || `Erro: ${error.status}`;
            notificationService.error(errorMessage);
        }
      }

      console.error('HTTP Error:', {
        status: error.status,
        message: errorMessage,
        url: req.url,
        hasToken: !!storageService.getToken()
      });

      return throwError(() => new Error(errorMessage));
    })
  );
};
