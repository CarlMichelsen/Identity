<script lang="ts">
    import type { User } from "../../model/user";
    import { UserClient } from "../../util/clients/userClient";
    import { hostUrl } from "../../util/endpoints";
    const developmentLoginRedirectUrl = `${hostUrl()}/api/v1/login/development?dest=${location.href}`;
    const client = new UserClient();

    type AuthenticationStatus = {
        status: "pending"|"loggedout"|"loggedin"
        user?: User;
    }

    let user = $state<AuthenticationStatus>({ status: "pending" });

    const checkLogin = async () => {
        const userResponse = await client.user();
        if (userResponse.ok && userResponse.value) {
            user.status = "loggedin";
            user.user = userResponse.value;
            return;
        }

        user.status = "loggedout";
    }

    const refresh = async () => {
        const res = await client.refresh();
        if (res.ok) {
            await checkLogin();
        }
    }

    const logout = async () => {
        const logoutResponse = await client.logout();
        if (!logoutResponse.ok) {
            throw new Error(logoutResponse.errors.join('\n'));
        }

        user.status = "loggedout";
        delete user.user;
    }

    checkLogin();
</script>

<p class="text-xl">Home page</p>

{#if user.status === "pending"}
    <p>loading...</p>
{:else if user.status === "loggedin"}
    <button onclick={() => logout()}>logout</button>
{:else}
    <a href={developmentLoginRedirectUrl}>Development login</a>
    <br>
    <button onclick={() => refresh()}>refresh</button>
{/if}