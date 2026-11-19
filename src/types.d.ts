declare module '@angular/core' {
  export function Component(options: any): ClassDecorator;
  export function Injectable(options?: any): ClassDecorator;
}

declare module '@angular/common' {
  export const CommonModule: any;
}

declare module '@angular/forms' {
  export const FormsModule: any;
}
