# Introduction #

SMS messages send by users should have following format:
> `<COMMAND> [parameter] [,...n]`

Commands text is case-insensitive.

# List of supported commands #
  * ## Balance|Bal|Баланс|Бал ##
Has no parameters. Returns current balance on user account.

**Not implemented yet**

  * ## Transfer|Tran|Перевод|Пер ##
Transfers money from user's account to another user account.
Parameters:
  1. destination account number
  1. sum
  1. source account number (optional parameter. Should be used when SMS is being send from unregistered phone number)

**Examples:**

`Transfer 12345 300`

`Transfer 12345 300 54321`

**Not implemented yet**


_More commands will be added soon._