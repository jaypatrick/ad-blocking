/**
 * Base menu class
 */

import inquirer from 'inquirer';
import { showError, showHeader } from '../utils.ts';

/** Menu action type */
export type MenuAction = () => Promise<void>;

/** Menu item */
export interface MenuItem {
  name: string;
  action: MenuAction;
}

/** Base menu class */
export abstract class BaseMenu {
  protected abstract get title(): string;
  protected abstract getMenuItems(): MenuItem[];

  /** Show the menu and handle selection */
  async show(): Promise<void> {
    while (true) {
      showHeader(this.title);

      const items = this.getMenuItems();
      const choices = [
        ...items.map((item) => item.name),
        '← Back',
      ];

      const { choice } = await inquirer.prompt([
        {
          type: 'list',
          name: 'choice',
          message: 'Select an option:',
          choices,
        },
      ]);

      if (choice === '← Back') {
        return;
      }

      const selectedItem = items.find((item) => item.name === choice);
      if (selectedItem) {
        try {
          await selectedItem.action();
        } catch (error) {
          showError(error instanceof Error ? error.message : String(error));
        }

        // Wait for user to see results
        await this.waitForEnter();
      }
    }
  }

  /** Wait for user to press Enter */
  protected async waitForEnter(): Promise<void> {
    await inquirer.prompt([
      {
        type: 'input',
        name: 'continue',
        message: 'Press Enter to continue...',
      },
    ]);
  }

  /** Confirm an action */
  protected async confirm(message: string): Promise<boolean> {
    const { confirmed } = await inquirer.prompt([
      {
        type: 'confirm',
        name: 'confirmed',
        message,
        default: false,
      },
    ]);
    return confirmed;
  }

  /** Select from a list */
  protected async selectItem<T>(
    message: string,
    items: T[],
    formatter: (item: T) => string,
  ): Promise<T | undefined> {
    if (items.length === 0) {
      return undefined;
    }

    const choices = items.map((item) => ({
      name: formatter(item),
      value: item,
    }));

    const { selected } = await inquirer.prompt([
      {
        type: 'list',
        name: 'selected',
        message,
        choices: [...choices, { name: '← Cancel', value: undefined }],
      },
    ]);

    return selected;
  }

  /** Get text input */
  protected async getInput(
    message: string,
    defaultValue?: string,
  ): Promise<string> {
    const { value } = await inquirer.prompt([
      {
        type: 'input',
        name: 'value',
        message,
        default: defaultValue,
      },
    ]);
    return value;
  }
}
