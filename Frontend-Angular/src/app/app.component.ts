import { Component } from '@angular/core';
import { RouterOutlet, RouterModule } from '@angular/router';
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatIconModule} from '@angular/material/icon';
import {MatButtonModule} from '@angular/material/button';
import { MatNavList } from '@angular/material/list';
import { MatDivider } from '@angular/material/list';
import { MatListModule } from '@angular/material/list';

@Component({
  standalone: true,
  selector: 'app-root',
  imports: [RouterOutlet,RouterModule ,MatSidenavModule, MatToolbarModule, MatIconModule, MatButtonModule, MatNavList, MatDivider, MatListModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.scss'
})
export class AppComponent {
  title = 'Frontend-Angular';
}