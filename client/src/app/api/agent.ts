import axios, { AxiosError, AxiosResponse } from "axios";
import { toast } from "react-toastify";
import { router } from "../router/Routes";

const sleep = () => new Promise(resolve => setTimeout(resolve, 500));

axios.defaults.baseURL = "http://localhost:5000/api/";
// With both sides (API & client) of this element in place, then our browser will recevie the cookie it will set the cookie inside our application storage as well.
// so If I press the ADD TO CART button it will add cookie to the client side.
axios.defaults.withCredentials = true;

// arrow functions, they make it easier to keep our code concise.
const responseBody = (response: AxiosResponse) => response.data;

// function responseBodyFn(response: AxiosResponse) {
//     return response.data;
// }

axios.interceptors.response.use(async response  => {
    await sleep();
    return response;
}, (error : AxiosError) => {
    // console.log("caught by interceptor");
    const {data, status} = error.response as AxiosResponse;
    switch (status) {
        // validation error is alos 400 error
        case 400:
            if (data.errors) {
                const modelStateError: string[] = [];
                for (const key in data.errors) {
                    if (data.errors[key]) {
                        modelStateError.push(data.errors[key])
                    }
                }
                // Returns a new array with all sub-array elements concatenated into it recursively up to the specified depth.
                // When we threw the errors from this statement, we stop excution of our case 400 method, so it effectively not going to call  toast.error(data.title); before the break; line
                throw modelStateError.flat();
            }
            toast.error(data.title);
            break;
        case 401:
            toast.error(data.title);
            break;
        case 404:
            toast.error(data.title);
            break;
        case 500:
            const dataObj = JSON.parse(data);
            // toast.error(dataObj.title);
            //console.log(dataObj);
            router.navigate("/server-error", {state: {error : dataObj}});
            break;
        default:
            break;           
    }
    return Promise.reject(error.response);
});

const requests = {
    get: (url: string) => axios.get(url).then(responseBody),
    post: (url: string, body: {}) => axios.post(url).then(responseBody),
    put: (url: string, body: {}) => axios.put(url).then(responseBody),
    delete: (url: string) => axios.delete(url).then(responseBody)
}

const TestErrors = {
    get400Error: () => requests.get("buggy/bad-request"),
    get401Error: () => requests.get("buggy/unauthorised"),
    get404Error: () => requests.get("buggy/not-found"),
    get500Error: () => requests.get("buggy/server-error"),
    getValidationError: () => requests.get("buggy/validation-error"),
}

const Catalog = {
    list: () => requests.get('products'),
    detail: (id: number) => requests.get(`products/${id}`)
}

const Basket = {
    // the parameter is from cookie
    // it will request parameter from cookie.
    get: () => requests.get("basket"),
    addItem: (productId: number, quantity = 1) => requests.post(`basket?productId=${productId}&quantity=${quantity}`, {}),
    removeItem: (productId: number, quantity = 1) => requests.delete(`basket?productId=${productId}&quantity=${quantity}`)
}

const agent = {
    Catalog,
    TestErrors,
    Basket
}

export default agent;