<script lang="ts">
    import type { User } from "../../model/user";
    import UserCard from "./UserCard.svelte";

    type UserSelectorProps = {
        options: User[];
        selected: (option: User) => void;
    }

    let { options, selected }: UserSelectorProps = $props();
</script>


<ol>
    {#each options as option}
        <li class="inline-block">
            <UserCard user={option} select={selected} />
        </li>
    {/each}
</ol>