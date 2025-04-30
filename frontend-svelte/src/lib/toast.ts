import toast from 'svelte-french-toast';

// Standard toast duration in milliseconds
const DURATION = 5000;

// Success toast with default styling
export function showSuccess(message: string) {
  toast.success(message, {
    duration: DURATION,
    position: 'top-right',
    style: 'background: #f0fdf4; color: #166534; border: 1px solid #bbf7d0;'
  });
}

// Error toast with default styling
export function showError(message: string) {
  toast.error(message, {
    duration: DURATION,
    position: 'top-right',
    style: 'background: #fef2f2; color: #b91c1c; border: 1px solid #fecaca;'
  });
}

// Info toast with default styling
export function showInfo(message: string) {
  toast(message, {
    duration: DURATION,
    position: 'top-right',
    style: 'background: #f0f9ff; color: #0369a1; border: 1px solid #bae6fd;'
  });
}