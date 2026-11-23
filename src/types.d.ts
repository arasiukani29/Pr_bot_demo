declare module '@angular/core' {
  export function Component(options: Record<string, unknown>): ClassDecorator;
  export function Injectable(options?: Record<string, unknown>): ClassDecorator;
}

declare module '@angular/common' {
  export const CommonModule: any;
}

declare module '@angular/forms' {
  export const FormsModule: any;
}
