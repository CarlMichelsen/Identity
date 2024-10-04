<script lang="ts">
  import { Router, Route } from "svelte-routing";
  import Root from "./lib/Root.svelte";
  import Home from "./lib/Pages/Home.svelte";
    import Idp from "./lib/Pages/Idp.svelte";

  type AppProps = {
    url: string;
  };

  let { url }: AppProps = $props();
</script>

<Root>
  <Router {url}>
    <Route path="/">
      <Home />
    </Route>

    <Route path="/idp">
      <Idp />
    </Route>
  </Router>
</Root>
