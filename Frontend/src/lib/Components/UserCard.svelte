<script lang="ts">
    import type { User } from "../../model/user";

    type UserCardProps = {
        user: User;
        select: (user: User) => void;
    }

    let { user, select }: UserCardProps = $props();
</script>

<button
    class="px-2 py-0.5 bg-green-300 hover:bg-green-600 hover:underline m-1 rounded-sm grid grid-cols-[1fr_2rem] gap-2"
    onclick={() => select(user)}>
    <p class="text-lg">{user.username}</p>
    <img src={user.avatarUrl} alt="profile">
    <p class="text-xs col-span-2">{user.email}</p>
</button>