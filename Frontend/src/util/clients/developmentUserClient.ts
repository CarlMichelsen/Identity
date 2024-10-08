import type { LoginResponse } from "../../model/loginResponse";
import type { ServiceResponse } from "../../model/serviceResponse";
import type { User } from "../../model/user";
import { BaseClient } from "../baseClient";

export class DevelopmentUserClient extends BaseClient
{
    private readonly developmentLoginPath = "api/v1/development";

    public async login(testUserId: number): Promise<ServiceResponse<LoginResponse>>
    {
        const state = new URLSearchParams(location.search).get('state');
        return await this.request<LoginResponse>(
            "POST",
            `${this.developmentLoginPath}/registerUser/${testUserId}?state=${state}`);
    }

    public async users(): Promise<ServiceResponse<User[]>>
    {
        return await this.request<User[]>(
            "GET",
            `${this.developmentLoginPath}/users`);
    }
}