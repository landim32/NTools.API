import UserInfo from '../../DTO/Domain/UserInfo';
import StatusRequest from '../../DTO/Services/StatusRequest';
import StringResult from '../../DTO/Services/StringResult';
import UserListResult from '../../DTO/Services/UserListResult';
import UserResult from '../../DTO/Services/UserResult';
import UserTokenResult from '../../DTO/Services/UserTokenResult';
import IHttpClient from '../../Infra/Interface/IHttpClient';
import IUserService from '../Interfaces/IUserService';

//const API_URL = "https://emagine.com.br/auth-api"; 
let _httpClient: IHttpClient;

const UserService: IUserService = {

  init: function (httpClient: IHttpClient): void {
    _httpClient = httpClient;
  },
  uploadImageUser: async (file: Blob, token: string) => {
    let ret = {} as StringResult;

    const formData = new FormData();
    formData.append('file', file, 'cropped.jpg');

    //formData.append("networkId", "0");
    const request = await _httpClient.doPostFormDataAuth<StringResult>('/uploadImageUser', formData, token);
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  getMe: async (token: string) => {
    let ret = {} as UserResult;
    const request = await _httpClient.doGetAuth<UserResult>('/getMe', token);
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  getUserByEmail: async (email: string) => {
    let ret = {} as UserResult;
    const request = await _httpClient.doGet<UserResult>('/getByEmail/' + email, {});
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  getBySlug: async (slug: string) => {
    let ret = {} as UserResult;
    const request = await _httpClient.doGet<UserResult>('/getBySlug/' + slug, {});
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  insert: async (user: UserInfo) => {
    let ret = {} as UserResult;
    const request = await _httpClient.doPost<UserResult>('/insert', user);
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  update: async (user: UserInfo, token: string) => {
    let ret = {} as UserResult;
    const request = await _httpClient.doPostAuth<UserResult>('/update', user, token);
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  loginWithEmail: async (email: string, password: string) => {
    let ret = {} as UserTokenResult;
    const request = await _httpClient.doPost<UserTokenResult>('/loginWithEmail', {
      email: email,
      password: password,
    });
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  hasPassword: async (token: string) => {
    let ret = {} as StatusRequest;
    const request = await _httpClient.doGetAuth<StatusRequest>('/hasPassword', token);
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  changePassword: async (oldPassword: string, newPassword: string, token: string) => {
    let ret = {} as StatusRequest;
    const request = await _httpClient.doPostAuth<StatusRequest>(
      '/changePassword',
      {
        oldPassword: oldPassword,
        newPassword: newPassword,
      },
      token,
    );
    console.log('request: ', request);
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  sendRecoveryEmail: async (email: string) => {
    let ret = {} as StatusRequest;
    const request = await _httpClient.doGet<StatusRequest>('/sendRecoveryMail/' + email, {});
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  changePasswordUsingHash: async (recoveryHash: string, newPassword: string) => {
    let ret = {} as StatusRequest;
    const request = await _httpClient.doPost<StatusRequest>('/changePasswordUsingHash', {
      recoveryHash: recoveryHash,
      newPassword: newPassword,
    });
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  },
  list: async (take: number) => {
    let ret = {} as UserListResult;
    const request = await _httpClient.doGet<UserResult>('/list/' + take, {});
    if (request.success) {
      return request.data;
    } else {
      ret = {
        ...ret,
        mensagem: request.messageError,
        sucesso: false,
      };
    }
    return ret;
  }
};

export default UserService;