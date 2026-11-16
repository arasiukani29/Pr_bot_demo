import { bootstrapApplication } from '@angular/platform-browser';
import { TableComponent } from './app/table.component';

bootstrapApplication(TableComponent)
  .catch((err) => console.error(err));
