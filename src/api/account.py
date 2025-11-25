import requests
from typing import Optional

def generate_token(base_url_account: str, username: str, password: str, timeout: int = 10) -> str:
    """
    Request a bearer token from Account service.

    Note:
        This function expects `base_url_account` to be the base to which the
        endpoint path is appended. For example, if your `.env` contains
        `BASE_URL_ACCOUNT=https://bookstore.toolsqa.com/Account/v1`, this
        function will build the final URL by appending `/GenerateToken`.
        If the provided base is incorrect (missing the versioned path), the
        request will fail -- this behavior is intentional so configuration
        issues surface early.
    """
    
    url = base_url_account.rstrip("/") + "/GenerateToken"
    resp = requests.post(url, json={"userName": username, "password": password}, timeout=timeout)
    resp.raise_for_status()
    data = resp.json()
    token = data.get("token")
    if not token:
        raise RuntimeError(f"Token missing in response: {data}")
    return token