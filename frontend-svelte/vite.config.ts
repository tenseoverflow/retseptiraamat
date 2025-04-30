import tailwindcss from '@tailwindcss/vite';
import { sveltekit } from '@sveltejs/kit/vite';
import { defineConfig } from 'vite';

export default defineConfig({
	plugins: [tailwindcss(), sveltekit()],
	define: {
		// Make environment variables available to the client
		'import.meta.env.API_URL': JSON.stringify(process.env.API_URL || 'http://localhost:5036')
	}
});
