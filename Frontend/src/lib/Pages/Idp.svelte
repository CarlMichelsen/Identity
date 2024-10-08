<script lang="ts">
    import type { User } from "../../model/user";
    import { DevelopmentUserClient } from "../../util/clients/developmentUserClient";
    import UserSelector from "../Components/UserSelector.svelte";

    const login = async (developmentUserId: number) => {
        const client = new DevelopmentUserClient();
        const res = await client.login(developmentUserId);
        if (!res.ok) {
            console.error(res.errors);
            return;
        }
        
        location.replace(res.value!.redirect);
    }

    let users = $state<User[] | null>(null);

    const getUsers = async () => {
        const client = new DevelopmentUserClient();
        var userResponse = await client.users();
        if (!userResponse.ok) {
            console.error(userResponse.errors);
            return null;
        }

        users = userResponse.value ?? null;
    }
    
    getUsers();
</script>

<h1 class="text-xl">Idp page</h1>

{#if users !== null}
    <UserSelector options={users} selected={user => login(Number(user.id))} />
{:else}
    <p>Loading test users...</p>
{/if}