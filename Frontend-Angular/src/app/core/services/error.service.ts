import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class ErrorService {

  private _error = new BehaviorSubject<string | null>(null);

  error$ = this._error.asObservable();

  setError(message: string) {
    this._error.next(message);
  }

  clear() {
    this._error.next(null);
  }
}