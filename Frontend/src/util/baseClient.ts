import { v4 as uuidv4 } from 'uuid';
import type { ServiceResponse } from "../model/serviceResponse";
import { hostUrl } from "./endpoints";

export type Method = "GET"
    | "HEAD"
    | "POST"
    | "PUT"
    | "DELETE"
    | "CONNECT"
    | "OPTIONS"
    | "TRACE"
    | "PATCH";

export abstract class BaseClient
{
    private static host: string = hostUrl();

    protected async request<T>(
        method: Method,
        path: string,
        body?: object,
        headers?: { [key: string]: string },
        traceId?: string)
    {
        return await this.internalRequest<T>(method, path, body, headers, traceId);
    }

    private async internalRequest<T>(
        method: Method,
        path: string,
        body?: object,
        headers?: { [key: string]: string },
        traceId?: string,): Promise<ServiceResponse<T>>
    {
        const actualTraceId = traceId ?? uuidv4();
        const init: RequestInit = {
            method: method,
            credentials: 'include',
            headers: {
                "Content-Type": "application/json",
                "X-Trace-Id": actualTraceId,
                ...headers },
        };

        if (body) {
            init.body = JSON.stringify(body);
        }

        const fullDestination = BaseClient.host + this.ensureLeadingSlash(path);
        const response = await fetch(fullDestination, init);

        if (response.ok) {
            return (await response.json()) as ServiceResponse<T>;
        } else {
            throw new Error(`${method} "${fullDestination}" request failed with status: ${response.status}`);
        }
    }

    private ensureLeadingSlash(input: string): string {
        if (!input.startsWith("/")) {
            return "/" + input;
        }

        return input;
    }
}