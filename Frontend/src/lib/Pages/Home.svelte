<script lang="ts">
    import { hostUrl } from "../../util/endpoints";
    const devLogin = `${hostUrl()}/api/v1/login/development?dest=${location.href}`;
</script>

<p class="text-xl">Home page</p>

<a href={devLogin}>Development login</a>