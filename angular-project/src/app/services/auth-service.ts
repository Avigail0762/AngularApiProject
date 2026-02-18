import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { jwtDecode } from 'jwt-decode';
import { of } from 'rxjs';
import { tap, map, catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})

export class AuthService {
  httpClient: HttpClient = inject(HttpClient);
  BASE_URL: string = 'https://localhost:7253/api/auth';

  isLoggedIn: boolean = false;

  login(username: string, password: string) {
  return this.httpClient
    .post<{ token: string; userId?: number; id?: number }>(this.BASE_URL + '/login', { username, password })
    .pipe(
      tap((response) => {
        const cleaned = (response?.token ?? '').replace(/^Bearer\s+/i, '');
        localStorage.setItem('authToken', cleaned);
        // Prefer explicit id returned by server; fallback to JWT decoding
        const explicit = response?.userId ?? response?.id;
        if (explicit !== undefined && explicit !== null) {
          const num = Number(explicit);
          if (Number.isFinite(num)) localStorage.setItem('userId', String(num));
        } else {
          const uid = this.getUserId();
          if (uid !== null) localStorage.setItem('userId', String(uid));
        }
        const role = this.getRole();
        localStorage.setItem('role', role);
        window.dispatchEvent(new CustomEvent('authTokenUpdated'));
        this.isLoggedIn = true;
      }),
      map(() => true),
      catchError((err) => {
        this.isLoggedIn = false;
        alert('שגיאה בהתחברות: ' + err.message);
        return of(false);
      })
    );
}

  getRole(): 'manager' | 'user' | 'userWithoutToken' {
    const token = localStorage.getItem('authToken');
    if (!token) return 'userWithoutToken';
    try {
      const decoded: any = jwtDecode(token.replace(/^Bearer\s+/i, ''));
      const keys = [
        'Role',
        'role',
        'roles',
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/role',
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role'
      ];
      let role: string | null = null;
      for (const key of keys) {
        const val = decoded?.[key];
        if (Array.isArray(val) && val.length) { role = String(val[0]); break; }
        if (typeof val === 'string' && val) { role = val; break; }
      }
      role = (role ?? '').toLowerCase();
      return role === 'manager' ? 'manager' : 'user';
    } catch {
      return 'user';
    }
  }

  isManager(): boolean { return this.getRole() === 'manager'; }

  getStoredRole(): 'manager' | 'user' | 'userWithoutToken' {
    const stored = localStorage.getItem('role');
    if (stored === 'manager' || stored === 'user') return stored;
    return this.getRole();
  }

  getUserId(): number | null {
    const token = localStorage.getItem('authToken');
    if (!token) return null;
    try {
      const decoded: any = jwtDecode(token.replace(/^Bearer\s+/i, ''));
      const keys = [ //לבדוק האם אפשר לקצר כנ"ל לשאר הפונקציות
        // סטנדרטי
        'sub',
        'nameid',
        'http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier',
        'http://schemas.microsoft.com/ws/2008/06/identity/claims/nameidentifier',
        // נפוצים במימושים שונים
        'Id',
        'ID',
        'UserId',
        'userId',
        'userid'
      ];
      for (const key of keys) {
        const val = decoded?.[key];
        if (val !== undefined && val !== null) {
          const num = Number(val);
          return Number.isFinite(num) ? num : null;
        }
      }
      // נסיון אחרון: מציאת שדה שמכיל 'id' והערך שלו מספרי
      for (const [k, v] of Object.entries(decoded as Record<string, unknown>)) {
        if (k && k.toLowerCase().includes('id')) {
          const num = Number(v as any);
          if (Number.isFinite(num)) return num;
        }
      }
      return null;
    } catch {
      return null;
    }
  }


  getStoredUserId(): number | null {
    const stored = localStorage.getItem('userId');
    if (stored !== null) {
      const num = Number(stored);
      if (Number.isFinite(num)) return num;
    }
    return this.getUserId();
  }

  hasToken(): boolean {
    const token = localStorage.getItem('authToken');
    return !!token;
  }

  logout() {
    localStorage.removeItem('authToken');
    localStorage.removeItem('role');
    localStorage.removeItem('userId');
    window.dispatchEvent(new CustomEvent('authTokenUpdated'));
    alert('התנתקת מהמערכת.');
  }
}
