import type { ServiceResponse } from "../../model/serviceResponse";
import type { User } from "../../model/user";
import { BaseClient } from "../baseClient";

export class UserClient extends BaseClient
{
    private readonly userPath = "api/v1/user";

    public async user(): Promise<ServiceResponse<User>>
    {
        return await this.request<User>(
            "GET",
            this.userPath);
    }

    public async refresh(): Promise<ServiceResponse<void>>
    {
        return await this.request<void>(
            "PUT",
            this.userPath);
    }

    public async logout(): Promise<ServiceResponse<void>>
    {
        return await this.request<void>(
            "DELETE",
            this.userPath);
    }
}