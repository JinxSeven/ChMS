const ACCESS_TOKEN_NAME = "access-token-chms";

export const setAccessToken = (token: string) => {
  document.cookie = `${ACCESS_TOKEN_NAME}=${token}; path=/; SameSite=Strict`;
};

export const getAccessToken = () => {
  const tokenName = ACCESS_TOKEN_NAME + "=";
  const decoded = decodeURIComponent(document.cookie);
  const arr = decoded.split("; ");
  let res;
  arr.forEach((val) => {
    if (val.indexOf(tokenName) === 0) res = val.substring(tokenName.length);
  });
  return res;
};
