import { mount } from 'svelte';
import './app.css'
import App from './App.svelte'

const root: HTMLElement = document.getElementById("app")!;
const app = mount(App, { target: root, events: { event: () => console.log("mounted") } });

export default app;