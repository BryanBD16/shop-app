import { computed, effect, Injectable, signal } from '@angular/core';

const THEME_MODE = 'theme-mode';
type ThemeMode = 'light' | 'dark';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {

  private systemPrefersDark = window.matchMedia?.('(prefers-color-scheme: dark)')?.matches ?? false;

  public mode = signal<ThemeMode>(
    this.systemPrefersDark ? 'dark' : 'light'
  );

  public currentTheme = computed(() => this.mode());

  constructor() {

    const stored = localStorage.getItem(THEME_MODE);

    if (stored === 'light' || stored === 'dark') {
      this.mode.set(stored);
    }

    effect(() => {
      const theme = this.currentTheme();

      document.documentElement.classList.remove('light', 'dark');
      document.documentElement.classList.add(theme);

      localStorage.setItem(THEME_MODE, theme);
    });

    this.listenToSystemThemeChanges();
  }

  setMode(value: ThemeMode) {
    this.mode.set(value);
  }

  private listenToSystemThemeChanges() {
    const mediaQuery = window.matchMedia('(prefers-color-scheme: dark)');

    mediaQuery.addEventListener('change', (event) => {
      const stored = localStorage.getItem(THEME_MODE);

      // only auto-update if user has NOT manually chosen a theme
      if (stored !== 'light' && stored !== 'dark') {
        this.mode.set(event.matches ? 'dark' : 'light');
      }
    });
  }
}